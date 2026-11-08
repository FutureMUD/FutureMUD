#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical06Stock
{
	public static CatalogueItemDefinition MedievalWritingDoubleCeramicInkwell() => new()
	{
		Metadata = new("medieval_writing_double_ceramic_inkwell", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3998", []),
		Noun = "inkwell",
		ShortDescription = "a double ceramic inkwell",
		LongDescription = null,
		FullDescription = "This double ceramic inkwell is a small vessel with a narrow mouth and stable base for holding writing ink. Dark staining gathers around the lip where pens and brushes have been dipped repeatedly. It is shaped to sit beside a page, tablet, or desk while keeping the ink pooled and accessible.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
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

	public static CatalogueItemDefinition MedievalWritingSlateInkstone() => new()
	{
		Metadata = new("medieval_writing_slate_inkstone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4025", []),
		Noun = "inkstone",
		ShortDescription = "a slate inkstone",
		LongDescription = null,
		FullDescription = "This slate inkstone is a flat stone with a shallow grinding area and a small hollow for ink. The surface is smoothed by use, with dark stains where an ink stick would be rubbed with water. It is a central tool for brush writing, calligraphy, and careful ink preparation.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "slate",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Calligraphy Tools"
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

	public static CatalogueItemDefinition MedievalWritingStoneInkstone() => new()
	{
		Metadata = new("medieval_writing_stone_inkstone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4052", []),
		Noun = "inkstone",
		ShortDescription = "a polished stone inkstone",
		LongDescription = null,
		FullDescription = "This polished stone inkstone is a flat stone with a shallow grinding area and a small hollow for ink. The surface is smoothed by use, with dark stains where an ink stick would be rubbed with water. It is a central tool for brush writing, calligraphy, and careful ink preparation.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 700.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Calligraphy Tools"
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

	public static CatalogueItemDefinition MedievalWritingAgateInkstone() => new()
	{
		Metadata = new("medieval_writing_agate_inkstone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4079", []),
		Noun = "inkstone",
		ShortDescription = "a fine agate inkstone",
		LongDescription = null,
		FullDescription = "This fine agate inkstone is a flat stone with a shallow grinding area and a small hollow for ink. The surface is smoothed by use, with dark stains where an ink stick would be rubbed with water. It is a central tool for brush writing, calligraphy, and careful ink preparation.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "agate",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Calligraphy Tools"
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

	public static CatalogueItemDefinition MedievalWritingJadeInkstone() => new()
	{
		Metadata = new("medieval_writing_jade_inkstone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4106", []),
		Noun = "inkstone",
		ShortDescription = "a small jade inkstone",
		LongDescription = null,
		FullDescription = "This small jade inkstone is a flat stone with a shallow grinding area and a small hollow for ink. The surface is smoothed by use, with dark stains where an ink stick would be rubbed with water. It is a central tool for brush writing, calligraphy, and careful ink preparation.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 480.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Calligraphy Tools"
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

	public static CatalogueItemDefinition MedievalWritingPigmentShell() => new()
	{
		Metadata = new("medieval_writing_pigment_shell", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4133", []),
		Noun = "shell",
		ShortDescription = "a pigment mixing shell",
		LongDescription = null,
		FullDescription = "This pigment mixing shell is a small shallow surface for mixing ink, binder, and pigment in controlled amounts. Its hollow or wells are smooth and easy to wipe, with faint stains from repeated colours. It is meant for illumination, rubrication, and small-scale manuscript painting.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "shell",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Pigment Shell"
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

	public static CatalogueItemDefinition MedievalWritingShellPalette() => new()
	{
		Metadata = new("medieval_writing_shell_palette", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4160", []),
		Noun = "palette",
		ShortDescription = "a shallow shell palette",
		LongDescription = null,
		FullDescription = "This shallow shell palette is a small shallow surface for mixing ink, binder, and pigment in controlled amounts. Its hollow or wells are smooth and easy to wipe, with faint stains from repeated colours. It is meant for illumination, rubrication, and small-scale manuscript painting.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "shell",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Pigment Shell"
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

	public static CatalogueItemDefinition MedievalWritingStonePigmentMuller() => new()
	{
		Metadata = new("medieval_writing_stone_pigment_muller", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4187", []),
		Noun = "muller",
		ShortDescription = "a stone pigment muller",
		LongDescription = null,
		FullDescription = "This stone pigment muller is a heavy smooth tool for grinding pigment into a finer working paste. The working face is worn flat and carries faint stains from mineral colour, soot, and binder. It is used before ink, paint, or illumination colours are ready for a brush.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Pigment Muller"
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

	public static CatalogueItemDefinition MedievalWritingPaletteSlab() => new()
	{
		Metadata = new("medieval_writing_palette_slab", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4214", []),
		Noun = "slab",
		ShortDescription = "a stone palette slab",
		LongDescription = null,
		FullDescription = "This stone palette slab is a small shallow surface for mixing ink, binder, and pigment in controlled amounts. Its hollow or wells are smooth and easy to wipe, with faint stains from repeated colours. It is meant for illumination, rubrication, and small-scale manuscript painting.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Palette Slab"
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

	public static CatalogueItemDefinition MedievalWritingAgateBurnisher() => new()
	{
		Metadata = new("medieval_writing_agate_burnisher", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4241", []),
		Noun = "burnisher",
		ShortDescription = "an agate burnisher",
		LongDescription = null,
		FullDescription = "This agate burnisher is a compact writing or illumination supply prepared for use at a desk or scriptorium bench. The material is kept in a form that can be rubbed, dipped, mixed, or portioned as needed. It supports ink making, page preparation, colouring, and decorative manuscript work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 85.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "agate",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Agate Burnisher"
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

	public static CatalogueItemDefinition MedievalWritingGessoPot() => new()
	{
		Metadata = new("medieval_writing_gesso_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4268", []),
		Noun = "pot",
		ShortDescription = "a small gesso pot",
		LongDescription = null,
		FullDescription = "This small gesso pot is a compact writing or illumination supply prepared for use at a desk or scriptorium bench. The material is kept in a form that can be rubbed, dipped, mixed, or portioned as needed. It supports ink making, page preparation, colouring, and decorative manuscript work.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Gesso Pot"
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

	public static CatalogueItemDefinition MedievalWritingGildingKnife() => new()
	{
		Metadata = new("medieval_writing_gilding_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4295", []),
		Noun = "knife",
		ShortDescription = "a gilding knife",
		LongDescription = null,
		FullDescription = "This gilding knife is a compact writing or illumination supply prepared for use at a desk or scriptorium bench. The material is kept in a form that can be rubbed, dipped, mixed, or portioned as needed. It supports ink making, page preparation, colouring, and decorative manuscript work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 75.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Gilding Knife"
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

	public static CatalogueItemDefinition MedievalWritingGildingTip() => new()
	{
		Metadata = new("medieval_writing_gilding_tip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4322", []),
		Noun = "tip",
		ShortDescription = "a soft gilding tip",
		LongDescription = null,
		FullDescription = "This soft gilding tip is a compact writing or illumination supply prepared for use at a desk or scriptorium bench. The material is kept in a form that can be rubbed, dipped, mixed, or portioned as needed. It supports ink making, page preparation, colouring, and decorative manuscript work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 15.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "feather",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Gilding Tip"
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

	public static CatalogueItemDefinition MedievalWritingGoldLeafCushion() => new()
	{
		Metadata = new("medieval_writing_gold_leaf_cushion", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4349", []),
		Noun = "cushion",
		ShortDescription = "a gold-leaf cushion",
		LongDescription = null,
		FullDescription = "This gold-leaf cushion holds fragile sheets of thin gold leaf protected between small wrappers. The leaf catches light at the edges and is delicate enough to tear with careless handling. It is intended for manuscript illumination, initials, borders, and other fine decorative work.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 220.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Gold Leaf Cushion"
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

	public static CatalogueItemDefinition MedievalWritingMiniatureDetailBrush() => new()
	{
		Metadata = new("medieval_writing_miniature_detail_brush", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4376", []),
		Noun = "brush",
		ShortDescription = "a miniature detail brush",
		LongDescription = null,
		FullDescription = "This miniature detail brush is a small brush made for manuscript colour, ink, or fine decorative work. The handle is slim and the bristles are gathered to a controlled point for narrow strokes and tiny details. It belongs with pigments, palettes, and illumination tools rather than with heavy writing stock.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 18.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Miniature Detail Brush"
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

	public static CatalogueItemDefinition MedievalWritingCedarScrollTube() => new()
	{
		Metadata = new("medieval_writing_cedar_scroll_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4403", []),
		Noun = "tube",
		ShortDescription = "a cedar scroll tube",
		LongDescription = null,
		FullDescription = "This cedar scroll tube is a narrow cedar container shaped to protect rolled documents. The long body is smooth enough not to abrade the scroll, with a fitted mouth, flap, or cap implied by its closed form. It is sized for scrolls, maps, rolls, labels, or other slender document bundles.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 450.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Scroll_Tube"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBambooScrollTube() => new()
	{
		Metadata = new("medieval_writing_bamboo_scroll_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4432", []),
		Noun = "tube",
		ShortDescription = "a bamboo scroll tube",
		LongDescription = null,
		FullDescription = "This bamboo scroll tube is a narrow bamboo container shaped to protect rolled documents. The long body is smooth enough not to abrade the scroll, with a fitted mouth, flap, or cap implied by its closed form. It is sized for scrolls, maps, rolls, labels, or other slender document bundles.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Scroll_Tube"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLeatherScrollCase() => new()
	{
		Metadata = new("medieval_writing_leather_scroll_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4461", []),
		Noun = "case",
		ShortDescription = "a leather scroll case",
		LongDescription = null,
		FullDescription = "This leather scroll case is a narrow leather container shaped to protect rolled documents. The long body is smooth enough not to abrade the scroll, with a fitted mouth, flap, or cap implied by its closed form. It is sized for scrolls, maps, rolls, labels, or other slender document bundles.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 520.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Scroll_Tube"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSilkScrollWrapper() => new()
	{
		Metadata = new("medieval_writing_silk_scroll_wrapper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4490", []),
		Noun = "wrapper",
		ShortDescription = "a silk scroll wrapper",
		LongDescription = null,
		FullDescription = "This silk scroll wrapper is a soft protective covering made to fold around manuscripts, scrolls, or tablets. The textile lies flat but can be wrapped snugly, with enough overlap to shield corners and outer leaves from dust. It is a light document protector rather than a hard case.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 80.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLinenScrollWrap() => new()
	{
		Metadata = new("medieval_writing_linen_scroll_wrap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4519", []),
		Noun = "wrap",
		ShortDescription = "a linen scroll wrap",
		LongDescription = null,
		FullDescription = "This linen scroll wrap is a soft protective covering made to fold around manuscripts, scrolls, or tablets. The textile lies flat but can be wrapped snugly, with enough overlap to shield corners and outer leaves from dust. It is a light document protector rather than a hard case.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLeatherDocumentPouch() => new()
	{
		Metadata = new("medieval_writing_leather_document_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4548", []),
		Noun = "pouch",
		ShortDescription = "a leather document pouch",
		LongDescription = null,
		FullDescription = "This leather document pouch is a flat closable pouch shaped for folded documents and small writing supplies. Its body is flexible, with a flap or tucked closure that protects contents from dust and casual handling. It suits letters, seals, tablets, labels, or compact scribal tools.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLinenDocumentPouch() => new()
	{
		Metadata = new("medieval_writing_linen_document_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4577", []),
		Noun = "pouch",
		ShortDescription = "a linen document pouch",
		LongDescription = null,
		FullDescription = "This linen document pouch is a flat closable pouch shaped for folded documents and small writing supplies. Its body is flexible, with a flap or tucked closure that protects contents from dust and casual handling. It suits letters, seals, tablets, labels, or compact scribal tools.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingTabletWrap() => new()
	{
		Metadata = new("medieval_writing_tablet_wrap", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4606", []),
		Noun = "wrap",
		ShortDescription = "a padded tablet wrap",
		LongDescription = null,
		FullDescription = "This padded tablet wrap is a soft protective covering made to fold around manuscripts, scrolls, or tablets. The textile lies flat but can be wrapped snugly, with enough overlap to shield corners and outer leaves from dust. It is a light document protector rather than a hard case.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWaxTabletPouch() => new()
	{
		Metadata = new("medieval_writing_wax_tablet_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4635", []),
		Noun = "pouch",
		ShortDescription = "a wax-tablet pouch",
		LongDescription = null,
		FullDescription = "This wax-tablet pouch is a flat closable pouch shaped for folded documents and small writing supplies. Its body is flexible, with a flap or tucked closure that protects contents from dust and casual handling. It suits letters, seals, tablets, labels, or compact scribal tools.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBookSatchel() => new()
	{
		Metadata = new("medieval_writing_book_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4664", []),
		Noun = "satchel",
		ShortDescription = "a leather book satchel",
		LongDescription = null,
		FullDescription = "This leather book satchel is a larger carried document bag with room for books, tablets, packets, and writing tools. The body is broad and flat so papers are not crushed into a bundle, with reinforced edges where it is handled most. It is meant for couriers, clerks, notaries, students, or travelling scribes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Satchel"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingDocumentSatchel() => new()
	{
		Metadata = new("medieval_writing_document_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4693", []),
		Noun = "satchel",
		ShortDescription = "a document satchel",
		LongDescription = null,
		FullDescription = "This document satchel is a larger carried document bag with room for books, tablets, packets, and writing tools. The body is broad and flat so papers are not crushed into a bundle, with reinforced edges where it is handled most. It is meant for couriers, clerks, notaries, students, or travelling scribes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 640.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Satchel"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCourierDocumentSatchel() => new()
	{
		Metadata = new("medieval_writing_courier_document_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4722", []),
		Noun = "satchel",
		ShortDescription = "a courier document satchel",
		LongDescription = null,
		FullDescription = "This courier document satchel is a larger carried document bag with room for books, tablets, packets, and writing tools. The body is broad and flat so papers are not crushed into a bundle, with reinforced edges where it is handled most. It is meant for couriers, clerks, notaries, students, or travelling scribes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 760.0,
		Cost = 40.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Satchel"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingNotarySatchel() => new()
	{
		Metadata = new("medieval_writing_notary_satchel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4751", []),
		Noun = "satchel",
		ShortDescription = "a notary's document satchel",
		LongDescription = null,
		FullDescription = "This notary's document satchel is a larger carried document bag with room for books, tablets, packets, and writing tools. The body is broad and flat so papers are not crushed into a bundle, with reinforced edges where it is handled most. It is meant for couriers, clerks, notaries, students, or travelling scribes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Satchel"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPalmLeafWrapper() => new()
	{
		Metadata = new("medieval_writing_palm_leaf_wrapper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4780", []),
		Noun = "wrapper",
		ShortDescription = "a palm-leaf manuscript wrapper",
		LongDescription = null,
		FullDescription = "This palm-leaf manuscript wrapper is a soft protective covering made to fold around manuscripts, scrolls, or tablets. The textile lies flat but can be wrapped snugly, with enough overlap to shield corners and outer leaves from dust. It is a light document protector rather than a hard case.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSilkManuscriptWrapper() => new()
	{
		Metadata = new("medieval_writing_silk_manuscript_wrapper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4809", []),
		Noun = "wrapper",
		ShortDescription = "a silk manuscript wrapper",
		LongDescription = null,
		FullDescription = "This silk manuscript wrapper is a soft protective covering made to fold around manuscripts, scrolls, or tablets. The textile lies flat but can be wrapped snugly, with enough overlap to shield corners and outer leaves from dust. It is a light document protector rather than a hard case.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 100.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCypressBookBox() => new()
	{
		Metadata = new("medieval_writing_cypress_book_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4838", []),
		Noun = "box",
		ShortDescription = "a cypress book box",
		LongDescription = null,
		FullDescription = "This cypress book box is a sturdy cypress storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1800.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCedarBookBox() => new()
	{
		Metadata = new("medieval_writing_cedar_book_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4867", []),
		Noun = "box",
		ShortDescription = "a cedar book box",
		LongDescription = null,
		FullDescription = "This cedar book box is a sturdy cedar storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1700.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLacqueredBookBox() => new()
	{
		Metadata = new("medieval_writing_lacquered_book_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4896", []),
		Noun = "box",
		ShortDescription = "a lacquered book box",
		LongDescription = null,
		FullDescription = "This lacquered book box is a sturdy lacquered storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1500.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lacquer",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSmallArchiveBox() => new()
	{
		Metadata = new("medieval_writing_small_archive_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4925", []),
		Noun = "box",
		ShortDescription = "a small archive box",
		LongDescription = null,
		FullDescription = "This small archive box is a sturdy oak storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLargeArchiveChest() => new()
	{
		Metadata = new("medieval_writing_large_archive_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4954", []),
		Noun = "chest",
		ShortDescription = "a large archive chest",
		LongDescription = null,
		FullDescription = "This large archive chest is a sturdy oak storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Chest"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingDeedBox() => new()
	{
		Metadata = new("medieval_writing_deed_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:4983", []),
		Noun = "box",
		ShortDescription = "a deed storage box",
		LongDescription = null,
		FullDescription = "This deed storage box is a sturdy walnut storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2400.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingScriptureBox() => new()
	{
		Metadata = new("medieval_writing_scripture_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5012", []),
		Noun = "box",
		ShortDescription = "a scripture storage box",
		LongDescription = null,
		FullDescription = "This scripture storage box is a sturdy cypress storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2100.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealMatrixBox() => new()
	{
		Metadata = new("medieval_writing_seal_matrix_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5041", []),
		Noun = "box",
		ShortDescription = "a seal matrix box",
		LongDescription = null,
		FullDescription = "This seal matrix box is a sturdy cedar storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Seal_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWaxAndSealBox() => new()
	{
		Metadata = new("medieval_writing_wax_and_seal_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5070", []),
		Noun = "box",
		ShortDescription = "a wax and seal box",
		LongDescription = null,
		FullDescription = "This wax and seal box is a sturdy cedar storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1000.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Seal_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingMessengerTube() => new()
	{
		Metadata = new("medieval_writing_messenger_tube", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5099", []),
		Noun = "tube",
		ShortDescription = "a messenger document tube",
		LongDescription = null,
		FullDescription = "This messenger document tube is a narrow bamboo container shaped to protect rolled documents. The long body is smooth enough not to abrade the scroll, with a fitted mouth, flap, or cap implied by its closed form. It is sized for scrolls, maps, rolls, labels, or other slender document bundles.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 320.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Scroll_Tube"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCourierScrollCase() => new()
	{
		Metadata = new("medieval_writing_courier_scroll_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5128", []),
		Noun = "case",
		ShortDescription = "a courier scroll case",
		LongDescription = null,
		FullDescription = "This courier scroll case is a narrow leather container shaped to protect rolled documents. The long body is smooth enough not to abrade the scroll, with a fitted mouth, flap, or cap implied by its closed form. It is sized for scrolls, maps, rolls, labels, or other slender document bundles.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 600.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Container_Scroll_Tube"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSchoolTabletCase() => new()
	{
		Metadata = new("medieval_writing_school_tablet_case", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5157", []),
		Noun = "case",
		ShortDescription = "a school tablet case",
		LongDescription = null,
		FullDescription = "This school tablet case is a practical document container or protector made from wooden. Its shape is meant to keep writing goods grouped, covered, and easier to carry or shelve. It belongs with the storage side of scribal, courier, school, and archive work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingDocumentBookcaseShelves() => new()
	{
		Metadata = new("medieval_writing_document_bookcase_shelves", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5186", []),
		Noun = "shelves",
		ShortDescription = "portable document shelves",
		LongDescription = null,
		FullDescription = "These portable document shelves are compact shelves made for books, scroll tubes, document boxes, and bundled records. The tiers are open and shallow, allowing stored documents to be seen and reached without digging through a chest. They are portable enough for the catalogue but visually suited to an archive, school, or scriptorium.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7800.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Bookcase_Shelves"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLockingDocumentBox() => new()
	{
		Metadata = new("medieval_writing_locking_document_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5215", []),
		Noun = "box",
		ShortDescription = "a locking document box",
		LongDescription = null,
		FullDescription = "This locking document box is a sturdy oak storage piece made for documents, books, seals, or archive packets. It has a built-in lock and keyhole, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 3400.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Market / Household Goods / Luxury Wares"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Lockbox"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLockingDeedChest() => new()
	{
		Metadata = new("medieval_writing_locking_deed_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5244", []),
		Noun = "chest",
		ShortDescription = "a locking deed chest",
		LongDescription = null,
		FullDescription = "This locking deed chest is a sturdy oak storage piece made for documents, books, seals, or archive packets. It has a built-in lock and keyhole, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 180.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLockingArchiveChest() => new()
	{
		Metadata = new("medieval_writing_locking_archive_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5273", []),
		Noun = "chest",
		ShortDescription = "a locking archive chest",
		LongDescription = null,
		FullDescription = "This locking archive chest is a sturdy oak storage piece made for documents, books, seals, or archive packets. It has a built-in lock and keyhole, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 26000.0,
		Cost = 220.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingChanceryStrongChest() => new()
	{
		Metadata = new("medieval_writing_chancery_strong_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5302", []),
		Noun = "chest",
		ShortDescription = "a chancery strong chest",
		LongDescription = null,
		FullDescription = "This chancery strong chest is a sturdy iron storage piece made for documents, books, seals, or archive packets. It has a built-in lock and keyhole, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 62000.0,
		Cost = 680.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealOfficeLockbox() => new()
	{
		Metadata = new("medieval_writing_seal_office_lockbox", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5331", []),
		Noun = "lockbox",
		ShortDescription = "a seal office lockbox",
		LongDescription = null,
		FullDescription = "This seal office lockbox is a sturdy brass storage piece made for documents, books, seals, or archive packets. It has a built-in lock and keyhole, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBronzeSignetStamp() => new()
	{
		Metadata = new("medieval_writing_bronze_signet_stamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5360", []),
		Noun = "stamp",
		ShortDescription = "a bronze signet stamp",
		LongDescription = null,
		FullDescription = "This bronze signet stamp is a compact seal tool with a shaped face cut for making an authority impression. The metal surface is worn bright along the high points, while the recessed design is kept sharp enough to press into wax or clay. It can serve personal, household, office, or messenger authentication.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 75.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_BronzeSignet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBrassOfficeSeal() => new()
	{
		Metadata = new("medieval_writing_brass_office_seal", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5388", []),
		Noun = "seal",
		ShortDescription = "a brass office seal",
		LongDescription = null,
		FullDescription = "This brass office seal is a seal matrix with a flat working face and a solid grip or back. The design surface is kept clear of ornament that would blur the impression, while the body shows practical handling wear. It is used to mark wax, clay, or other sealing media with a recognizable authority sign.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 320.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_BrassOfficeSeal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingIronSealMatrix() => new()
	{
		Metadata = new("medieval_writing_iron_seal_matrix", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5416", []),
		Noun = "matrix",
		ShortDescription = "an iron seal matrix",
		LongDescription = null,
		FullDescription = "This iron seal matrix is a seal matrix with a flat working face and a solid grip or back. The design surface is kept clear of ornament that would blur the impression, while the body shows practical handling wear. It is used to mark wax, clay, or other sealing media with a recognizable authority sign.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 260.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_IronSealMatrix"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLeadSealMatrix() => new()
	{
		Metadata = new("medieval_writing_lead_seal_matrix", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5444", []),
		Noun = "matrix",
		ShortDescription = "a lead seal matrix",
		LongDescription = null,
		FullDescription = "This lead seal matrix is a heavy seal matrix or bulla-related tool meant for formal packets and durable impressions. The working face is firm and plain-edged, with enough weight to leave a confident mark in a prepared medium. It suits official records, charters, rolls, and secured archive goods.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 380.0,
		Cost = 65.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lead",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_LeadSealMatrix"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingIvoryHandledSealStamp() => new()
	{
		Metadata = new("medieval_writing_ivory_handled_seal_stamp", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5472", []),
		Noun = "stamp",
		ShortDescription = "an ivory-handled seal stamp",
		LongDescription = null,
		FullDescription = "This ivory-handled seal stamp is a seal matrix with a flat working face and a solid grip or back. The design surface is kept clear of ornament that would blur the impression, while the body shows practical handling wear. It is used to mark wax, clay, or other sealing media with a recognizable authority sign.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 120.0,
		Cost = 120.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivory",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"SealStamp_Medieval_BronzeSignet"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWaxSealCake() => new()
	{
		Metadata = new("medieval_writing_wax_seal_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5500", []),
		Noun = "cake",
		ShortDescription = "a wax seal cake",
		LongDescription = null,
		FullDescription = "This wax seal cake is a small lump or cake of wax kept for sealing documents and packets. The surface is smooth where it has been wrapped or warmed, and the colour reads dark enough to show an impressed design. It is ready to be softened, pressed, and marked by a seal stamp.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
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

	public static CatalogueItemDefinition MedievalWritingRedWaxSealCake() => new()
	{
		Metadata = new("medieval_writing_red_wax_seal_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5528", []),
		Noun = "cake",
		ShortDescription = "a red wax seal cake",
		LongDescription = null,
		FullDescription = "This red wax seal cake is a small lump or cake of wax kept for sealing documents and packets. The surface is smooth where it has been wrapped or warmed, and the colour reads dark enough to show an impressed design. It is ready to be softened, pressed, and marked by a seal stamp.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 60.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
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

	public static CatalogueItemDefinition MedievalWritingClaySealingLump() => new()
	{
		Metadata = new("medieval_writing_clay_sealing_lump", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5556", []),
		Noun = "lump",
		ShortDescription = "a lump of sealing clay",
		LongDescription = null,
		FullDescription = "This lump of sealing clay is an authentication good associated with seals, secured documents, and official handling. Its surface or closure point is shaped to carry a mark, impression, tie, or visible authority sign. It belongs with chancery, archive, courier, mercantile, or institutional document work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "clay",
		Tags = [
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
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

	public static CatalogueItemDefinition MedievalWritingLeadBullaBlank() => new()
	{
		Metadata = new("medieval_writing_lead_bulla_blank", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5584", []),
		Noun = "bulla",
		ShortDescription = "a blank lead bulla",
		LongDescription = null,
		FullDescription = "This blank lead bulla is a heavy seal matrix or bulla-related tool meant for formal packets and durable impressions. The working face is firm and plain-edged, with enough weight to leave a confident mark in a prepared medium. It suits official records, charters, rolls, and secured archive goods.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lead",
		Tags = [
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
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

	public static CatalogueItemDefinition MedievalWritingLeadSealTag() => new()
	{
		Metadata = new("medieval_writing_lead_seal_tag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5612", []),
		Noun = "tag",
		ShortDescription = "a lead seal tag",
		LongDescription = null,
		FullDescription = "This lead seal tag is a small tag prepared for a written mark and a seal impression. Its surface is compact but clear, with an attachment point for tying it to a bundle, chest, scroll, or packet. It is useful for archive control, messenger custody, and official identification.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lead",
		Tags = [
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
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

	public static CatalogueItemDefinition MedievalWritingSealCord() => new()
	{
		Metadata = new("medieval_writing_seal_cord", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5640", []),
		Noun = "cord",
		ShortDescription = "a seal cord",
		LongDescription = null,
		FullDescription = "This seal cord is an authentication good associated with seals, secured documents, and official handling. Its surface or closure point is shaped to carry a mark, impression, tie, or visible authority sign. It belongs with chancery, archive, courier, mercantile, or institutional document work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
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

	public static CatalogueItemDefinition MedievalWritingSealableParchmentCharter() => new()
	{
		Metadata = new("medieval_writing_sealable_parchment_charter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5668", []),
		Noun = "charter",
		ShortDescription = "a sealable parchment charter",
		LongDescription = null,
		FullDescription = "This sealable parchment charter is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 12.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface",
				"Sealable_Medieval_Parchment_Charter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealedParchmentRoll() => new()
	{
		Metadata = new("medieval_writing_sealed_parchment_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5699", []),
		Noun = "roll",
		ShortDescription = "a sealable parchment roll",
		LongDescription = null,
		FullDescription = "This sealable parchment roll is a rolled document form prepared with an outer tie or overlap for a seal. The outer layer protects the writing while presenting a clear place for wax, clay, or lead to hold the roll closed. It is suited to secured correspondence, charters, official copies, and archive transfers.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 150.0,
		Cost = 40.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface",
				"Sealable_Medieval_Parchment_Roll"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealableRagPaperLetter() => new()
	{
		Metadata = new("medieval_writing_sealable_rag_paper_letter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5730", []),
		Noun = "letter",
		ShortDescription = "a sealable rag-paper letter",
		LongDescription = null,
		FullDescription = "This sealable rag-paper letter is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Letter_Surface",
				"Sealable_Medieval_Rag_Paper_Letter"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingOfficialWrit() => new()
	{
		Metadata = new("medieval_writing_official_writ", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5761", []),
		Noun = "writ",
		ShortDescription = "a sealable official writ",
		LongDescription = null,
		FullDescription = "This sealable official writ is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 10.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface",
				"Sealable_Medieval_Official_Writ"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealableEastAsianScroll() => new()
	{
		Metadata = new("medieval_writing_sealable_east_asian_scroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5792", []),
		Noun = "scroll",
		ShortDescription = "a sealable paper handscroll",
		LongDescription = null,
		FullDescription = "This sealable paper handscroll is a rolled document form prepared with an outer tie or overlap for a seal. The outer layer protects the writing while presenting a clear place for wax, clay, or lead to hold the roll closed. It is suited to secured correspondence, charters, official copies, and archive transfers.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Scroll_Surface",
				"Sealable_Medieval_East_Asian_Scroll"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealablePalmLeafBundle() => new()
	{
		Metadata = new("medieval_writing_sealable_palm_leaf_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5823", []),
		Noun = "bundle",
		ShortDescription = "a sealable palm-leaf bundle",
		LongDescription = null,
		FullDescription = "This sealable palm-leaf bundle is a corded manuscript packet prepared so its tie can carry a seal. The long leaves are stacked neatly between covers or wraps, with the cord left accessible for authentication. It is meant to show whether the manuscript bundle has been opened since sealing.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 460.0,
		Cost = 62.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leaf",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Bundle",
				"Sealable_Medieval_Palm_Leaf_Bundle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWaxSealableDocument() => new()
	{
		Metadata = new("medieval_writing_wax_sealable_document", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5854", []),
		Noun = "document",
		ShortDescription = "a wax-sealable document",
		LongDescription = null,
		FullDescription = "This wax-sealable document is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface",
				"Sealable_Document_Wax"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingClaySealableDocument() => new()
	{
		Metadata = new("medieval_writing_clay_sealable_document", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5885", []),
		Noun = "document",
		ShortDescription = "a clay-sealable document",
		LongDescription = null,
		FullDescription = "This clay-sealable document is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "papyrus",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Sheet_Surface",
				"Sealable_Document_Clay"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealablePaperEnvelope() => new()
	{
		Metadata = new("medieval_writing_sealable_paper_envelope", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5916", []),
		Noun = "envelope",
		ShortDescription = "a sealable paper envelope",
		LongDescription = null,
		FullDescription = "This sealable paper envelope is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Letter_Surface",
				"Sealable_Envelope"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealableScroll() => new()
	{
		Metadata = new("medieval_writing_sealable_scroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5947", []),
		Noun = "scroll",
		ShortDescription = "a general sealable scroll",
		LongDescription = null,
		FullDescription = "This general sealable scroll is a rolled document form prepared with an outer tie or overlap for a seal. The outer layer protects the writing while presenting a clear place for wax, clay, or lead to hold the roll closed. It is suited to secured correspondence, charters, official copies, and archive transfers.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 130.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface",
				"Sealable_Scroll"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealableDocumentPouch() => new()
	{
		Metadata = new("medieval_writing_sealable_document_pouch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:5978", []),
		Noun = "pouch",
		ShortDescription = "a sealable document pouch",
		LongDescription = null,
		FullDescription = "This sealable document pouch is built with a closure point prepared to take a visible tamper seal. The flap, lid, or hasp gives a clear place for wax, clay, or lead to bind the opening after the contents are secured. It is intended for courier custody, archive storage, or official handling where broken sealing should be obvious.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 280.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Pouch",
				"Sealable_Medieval_Document_Pouch"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealableArchiveBox() => new()
	{
		Metadata = new("medieval_writing_sealable_archive_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6008", []),
		Noun = "box",
		ShortDescription = "a sealable archive box",
		LongDescription = null,
		FullDescription = "This sealable archive box is built with a closure point prepared to take a visible tamper seal. The flap, lid, or hasp gives a clear place for wax, clay, or lead to bind the opening after the contents are secured. It is intended for courier custody, archive storage, or official handling where broken sealing should be obvious.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2800.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box",
				"Sealable_Medieval_Archive_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealableArchiveChest() => new()
	{
		Metadata = new("medieval_writing_sealable_archive_chest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6038", []),
		Noun = "chest",
		ShortDescription = "a sealable archive chest",
		LongDescription = null,
		FullDescription = "This sealable archive chest is built with a closure point prepared to take a visible tamper seal. The flap, lid, or hasp gives a clear place for wax, clay, or lead to bind the opening after the contents are secured. It is intended for courier custody, archive storage, or official handling where broken sealing should be obvious.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 24000.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Chest",
				"Sealable_Medieval_Archive_Box"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWaxSealableContainer() => new()
	{
		Metadata = new("medieval_writing_wax_sealable_container", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6068", []),
		Noun = "box",
		ShortDescription = "a wax-sealable document box",
		LongDescription = null,
		FullDescription = "This wax-sealable document box is built with a closure point prepared to take a visible tamper seal. The flap, lid, or hasp gives a clear place for wax, clay, or lead to bind the opening after the contents are secured. It is intended for courier custody, archive storage, or official handling where broken sealing should be obvious.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box",
				"Sealable_Container_Wax"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingParchmentScrapingKnife() => new()
	{
		Metadata = new("medieval_writing_parchment_scraping_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6098", []),
		Noun = "knife",
		ShortDescription = "a parchment scraping knife",
		LongDescription = null,
		FullDescription = "This parchment scraping knife is a curved or narrow-edged tool for scraping wet hide into a smoother writing surface. The working edge is keen but controlled, with handling wear where pressure would be applied. It belongs in parchment preparation, especially thinning, smoothing, and cleaning skins on a frame.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Scraping Knife"
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

	public static CatalogueItemDefinition MedievalWritingParchmentLunellum() => new()
	{
		Metadata = new("medieval_writing_parchment_lunellum", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6125", []),
		Noun = "lunellum",
		ShortDescription = "a crescent parchment lunellum",
		LongDescription = null,
		FullDescription = "This crescent parchment lunellum is a curved or narrow-edged tool for scraping wet hide into a smoother writing surface. The working edge is keen but controlled, with handling wear where pressure would be applied. It belongs in parchment preparation, especially thinning, smoothing, and cleaning skins on a frame.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 180.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Lunellum"
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

	public static CatalogueItemDefinition MedievalWritingParchmentStretchingFrame() => new()
	{
		Metadata = new("medieval_writing_parchment_stretching_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6152", []),
		Noun = "frame",
		ShortDescription = "a parchment stretching frame",
		LongDescription = null,
		FullDescription = "This parchment stretching frame is part of the heavy preparation work for stretching and holding parchment stock. Its surfaces show practical wear from tension, damp hide, cord, and repeated workshop handling. It helps turn raw skin into a flatter, thinner, more reliable writing material.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Stretching Frame"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingParchmentLacingCord() => new()
	{
		Metadata = new("medieval_writing_parchment_lacing_cord", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6179", []),
		Noun = "cord",
		ShortDescription = "a parchment lacing cord",
		LongDescription = null,
		FullDescription = "This parchment lacing cord is part of the heavy preparation work for stretching and holding parchment stock. Its surfaces show practical wear from tension, damp hide, cord, and repeated workshop handling. It helps turn raw skin into a flatter, thinner, more reliable writing material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Lacing Cord"
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

	public static CatalogueItemDefinition MedievalWritingParchmentPegs() => new()
	{
		Metadata = new("medieval_writing_parchment_pegs", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6206", []),
		Noun = "pegs",
		ShortDescription = "a set of parchment pegs",
		LongDescription = null,
		FullDescription = "This set of parchment pegs is part of the heavy preparation work for stretching and holding parchment stock. Its surfaces show practical wear from tension, damp hide, cord, and repeated workshop handling. It helps turn raw skin into a flatter, thinner, more reliable writing material.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Pegs"
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

	public static CatalogueItemDefinition MedievalWritingParchmentFleshingBeam() => new()
	{
		Metadata = new("medieval_writing_parchment_fleshing_beam", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6233", []),
		Noun = "beam",
		ShortDescription = "a parchment fleshing beam",
		LongDescription = null,
		FullDescription = "This parchment fleshing beam is part of the heavy preparation work for stretching and holding parchment stock. Its surfaces show practical wear from tension, damp hide, cord, and repeated workshop handling. It helps turn raw skin into a flatter, thinner, more reliable writing material.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9500.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Fleshing Beam"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingParchmentWhiteningChalk() => new()
	{
		Metadata = new("medieval_writing_parchment_whitening_chalk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6260", []),
		Noun = "chalk",
		ShortDescription = "a lump of whitening chalk",
		LongDescription = null,
		FullDescription = "This lump of whitening chalk is a finishing material for preparing parchment before ink work. It is dry, pale, abrasive, or powdery in the places meant to touch the writing surface. It helps smooth, whiten, dry, or degrease the page so writing sits cleanly.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "chalk",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Whitening Chalk"
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

	public static CatalogueItemDefinition MedievalWritingParchmentPumice() => new()
	{
		Metadata = new("medieval_writing_parchment_pumice", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6287", []),
		Noun = "pumice",
		ShortDescription = "a piece of parchment pumice",
		LongDescription = null,
		FullDescription = "This piece of parchment pumice is a finishing material for preparing parchment before ink work. It is dry, pale, abrasive, or powdery in the places meant to touch the writing surface. It helps smooth, whiten, dry, or degrease the page so writing sits cleanly.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pumice",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Pumice"
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

	public static CatalogueItemDefinition MedievalWritingParchmentPounceBag() => new()
	{
		Metadata = new("medieval_writing_parchment_pounce_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6314", []),
		Noun = "bag",
		ShortDescription = "a parchment pounce bag",
		LongDescription = null,
		FullDescription = "This parchment pounce bag is a finishing material for preparing parchment before ink work. It is dry, pale, abrasive, or powdery in the places meant to touch the writing surface. It helps smooth, whiten, dry, or degrease the page so writing sits cleanly.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Pounce Bag"
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

	public static CatalogueItemDefinition MedievalWritingPapermakerMouldDeckle() => new()
	{
		Metadata = new("medieval_writing_papermaker_mould_deckle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6341", []),
		Noun = "mould",
		ShortDescription = "a mould and deckle",
		LongDescription = null,
		FullDescription = "This mould and deckle is a wooden papermaking frame with the removable edge needed to shape a sheet. The working face is broad and flat, meant to lift wet fibres evenly from a vat. It is central to forming handmade paper before pressing and drying.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1800.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Mould and Deckle"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPapermakerVat() => new()
	{
		Metadata = new("medieval_writing_papermaker_vat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6368", []),
		Noun = "vat",
		ShortDescription = "a papermaker's vat",
		LongDescription = null,
		FullDescription = "This papermaker's vat is a broad wooden vessel used for wet fibre and rag preparation. The interior is plain and work-worn, with room for soaking, beating, or stirring paper stock. It belongs to the messy production side of paper rather than to finished desk work.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22000.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Papermaker's Vat"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCouchingBlanket() => new()
	{
		Metadata = new("medieval_writing_couching_blanket", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6395", []),
		Noun = "blanket",
		ShortDescription = "a couching blanket",
		LongDescription = null,
		FullDescription = "This couching blanket is a thick absorbent textile used between wet paper sheets during pressing. Its surface is soft and slightly matted, meant to draw moisture away without tearing the new sheet. It is a papermaker's working material, not a household blanket.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Couching Blanket"
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

	public static CatalogueItemDefinition MedievalWritingPressFelt() => new()
	{
		Metadata = new("medieval_writing_press_felt", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6422", []),
		Noun = "felt",
		ShortDescription = "a papermaker's press felt",
		LongDescription = null,
		FullDescription = "This papermaker's press felt is a thick absorbent textile used between wet paper sheets during pressing. Its surface is soft and slightly matted, meant to draw moisture away without tearing the new sheet. It is a papermaker's working material, not a household blanket.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Press Felt"
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

	public static CatalogueItemDefinition MedievalWritingLayPress() => new()
	{
		Metadata = new("medieval_writing_lay_press", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6449", []),
		Noun = "press",
		ShortDescription = "a papermaker's lay press",
		LongDescription = null,
		FullDescription = "This papermaker's lay press is a wooden pressure tool for flattening sheets, gatherings, or newly made paper stock. The frame and boards are heavy and plain, built to squeeze evenly rather than decorate a room. It is used where wet or newly assembled writing materials need to dry flat.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Lay Press"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingRagSortingKnife() => new()
	{
		Metadata = new("medieval_writing_rag_sorting_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6476", []),
		Noun = "knife",
		ShortDescription = "a rag-sorting knife",
		LongDescription = null,
		FullDescription = "This rag-sorting knife is a practical papermaking tool used in preparing, shaping, pressing, or finishing sheet stock. Its construction is workmanlike, with visible wear from wet fibres, sizing, pressure, or repeated handling. It belongs in a paper workshop rather than a finished writing kit.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Rag Sorting Knife"
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

	public static CatalogueItemDefinition MedievalWritingRagBeatingTrough() => new()
	{
		Metadata = new("medieval_writing_rag_beating_trough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6503", []),
		Noun = "trough",
		ShortDescription = "a rag-beating trough",
		LongDescription = null,
		FullDescription = "This rag-beating trough is a broad wooden vessel used for wet fibre and rag preparation. The interior is plain and work-worn, with room for soaking, beating, or stirring paper stock. It belongs to the messy production side of paper rather than to finished desk work.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12000.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Rag Beating Trough"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPaperSizingBrush() => new()
	{
		Metadata = new("medieval_writing_paper_sizing_brush", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6530", []),
		Noun = "brush",
		ShortDescription = "a paper sizing brush",
		LongDescription = null,
		FullDescription = "This paper sizing brush is a broad working brush for applying sizing to paper or other writing stock. The handle is practical and the bristles are meant to spread liquid evenly over a sheet. It belongs to surface preparation before the material is ready to write on.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Paper Sizing Brush"
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

	public static CatalogueItemDefinition MedievalWritingGelatineSizingPot() => new()
	{
		Metadata = new("medieval_writing_gelatine_sizing_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6557", []),
		Noun = "pot",
		ShortDescription = "a sizing pot",
		LongDescription = null,
		FullDescription = "This sizing pot is a workshop tool for making or finishing writing goods. It is plainly made from durable material and shows the form needed for repeated hand work. It belongs in a scriptorium, paper shop, bindery, or document workshop.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Gelatine Sizing Pot"
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

	public static CatalogueItemDefinition MedievalWritingPaperBurnishingAgate() => new()
	{
		Metadata = new("medieval_writing_paper_burnishing_agate", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6584", []),
		Noun = "agate",
		ShortDescription = "a paper burnishing agate",
		LongDescription = null,
		FullDescription = "This paper burnishing agate is a smooth hard stone used to burnish paper into a more polished writing face. The working surface is rounded and glossy from repeated rubbing. It is used after drying or sizing to reduce roughness and help ink sit cleanly.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "agate",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Paper Burnishing Agate"
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

	public static CatalogueItemDefinition MedievalWritingWatermarkWire() => new()
	{
		Metadata = new("medieval_writing_watermark_wire", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6611", []),
		Noun = "wire",
		ShortDescription = "a watermark wire",
		LongDescription = null,
		FullDescription = "This watermark wire is a shaped brass wire intended to leave a light mark in handmade paper. The wire is thin, flexible, and careful rather than strong, with bends that form a simple device. It is fitted to papermaking equipment when a sheet needs an identifying watermark.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 20.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Watermark Wire"
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

	public static CatalogueItemDefinition MedievalWritingBookbindersNeedle() => new()
	{
		Metadata = new("medieval_writing_bookbinders_needle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6638", []),
		Noun = "needle",
		ShortDescription = "a bookbinder's needle",
		LongDescription = null,
		FullDescription = "This bookbinder's needle is a stout needle sized for thread, cord, and the thick work of manuscript binding. Its eye is broad enough for binding thread, and the point is made for guided holes rather than delicate cloth sewing. It is used to sew quires, endbands, covers, and support cords into a firm book structure.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Bookbinder's Needle"
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

	public static CatalogueItemDefinition MedievalWritingEndbandNeedle() => new()
	{
		Metadata = new("medieval_writing_endband_needle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6665", []),
		Noun = "needle",
		ShortDescription = "an endband needle",
		LongDescription = null,
		FullDescription = "This endband needle is a stout needle sized for thread, cord, and the thick work of manuscript binding. Its eye is broad enough for binding thread, and the point is made for guided holes rather than delicate cloth sewing. It is used to sew quires, endbands, covers, and support cords into a firm book structure.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 14.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Endband Needle"
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

	public static CatalogueItemDefinition MedievalWritingBookbindersPunch() => new()
	{
		Metadata = new("medieval_writing_bookbinders_punch", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6692", []),
		Noun = "punch",
		ShortDescription = "a bookbinder's punch",
		LongDescription = null,
		FullDescription = "This bookbinder's punch is a short pointed tool for opening clean holes in quires, boards, and covers. The grip and point are made for steady pressure so sewing stations can line up without tearing the material. It belongs on a binding bench beside needles, cords, and presses.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 110.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Bookbinder's Punch"
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

	public static CatalogueItemDefinition MedievalWritingBookbinderSewingFrame() => new()
	{
		Metadata = new("medieval_writing_bookbinder_sewing_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6719", []),
		Noun = "frame",
		ShortDescription = "a bookbinder's sewing frame",
		LongDescription = null,
		FullDescription = "This bookbinder's sewing frame is a wooden frame for holding support cords while quires are sewn into a book block. The uprights and crosspieces are plain and practical, marked by cord tension and repeated adjustment. It keeps a manuscript spine orderly during the slow work of binding.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2600.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Bookbinder's Sewing Frame"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSewingSupportCords() => new()
	{
		Metadata = new("medieval_writing_sewing_support_cords", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6746", []),
		Noun = "cords",
		ShortDescription = "a set of sewing support cords",
		LongDescription = null,
		FullDescription = "This set of sewing support cords is prepared to support the sewing of a book spine. The hemp cords are firm, slightly rough, and cut to useful lengths for anchoring gatherings. Once worked into a binding, they help hold the quires together under the cover.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 150.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Sewing Support Cords"
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

	public static CatalogueItemDefinition MedievalWritingBackingHammer() => new()
	{
		Metadata = new("medieval_writing_backing_hammer", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6773", []),
		Noun = "hammer",
		ShortDescription = "a bookbinder's backing hammer",
		LongDescription = null,
		FullDescription = "This bookbinder's backing hammer is a compact iron hammer used for shaping and settling a book spine. The working face is smooth enough not to shred the leather or paper it strikes. It is a specialised binding tool for backing, rounding, and controlled pressure work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Backing Hammer"
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

	public static CatalogueItemDefinition MedievalWritingLeatherParingKnife() => new()
	{
		Metadata = new("medieval_writing_leather_paring_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6800", []),
		Noun = "knife",
		ShortDescription = "a leather paring knife",
		LongDescription = null,
		FullDescription = "This leather paring knife is a thin sharp knife for paring leather and trimming binding materials. The blade is shaped for shaving fine layers rather than cutting with force. It is used to thin cover leather, clean edges, and prepare flexible joints.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Leather Paring Knife"
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
}
