#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical06Stock
{
	public static CatalogueItemDefinition MedievalWritingLyingPress() => new()
	{
		Metadata = new("medieval_writing_lying_press", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6827", []),
		Noun = "press",
		ShortDescription = "a bookbinder's lying press",
		LongDescription = null,
		FullDescription = "This bookbinder's lying press is a wooden pressure tool for flattening sheets, gatherings, or newly made paper stock. The frame and boards are heavy and plain, built to squeeze evenly rather than decorate a room. It is used where wet or newly assembled writing materials need to dry flat.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 11500.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Lying Press"
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

	public static CatalogueItemDefinition MedievalWritingBookPress() => new()
	{
		Metadata = new("medieval_writing_book_press", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6854", []),
		Noun = "press",
		ShortDescription = "a wooden book press",
		LongDescription = null,
		FullDescription = "This wooden book press is a wooden pressure tool for flattening sheets, gatherings, or newly made paper stock. The frame and boards are heavy and plain, built to squeeze evenly rather than decorate a room. It is used where wet or newly assembled writing materials need to dry flat.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 18000.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Book Press"
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

	public static CatalogueItemDefinition MedievalWritingBookPlough() => new()
	{
		Metadata = new("medieval_writing_book_plough", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6881", []),
		Noun = "plough",
		ShortDescription = "a bookbinder's plough",
		LongDescription = null,
		FullDescription = "This bookbinder's plough is a bookbinder's trimming tool guided against a press-held book block. The body is wooden with a cutting element set to shave the page edges gradually and evenly. It is used to square and neaten a bound manuscript's fore edge.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3200.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Book Plough"
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

	public static CatalogueItemDefinition MedievalWritingScrollRollerRod() => new()
	{
		Metadata = new("medieval_writing_scroll_roller_rod", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6908", []),
		Noun = "rod",
		ShortDescription = "a scroll roller rod",
		LongDescription = null,
		FullDescription = "This scroll roller rod is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll Roller Rod"
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

	public static CatalogueItemDefinition MedievalWritingScrollSmoothingStone() => new()
	{
		Metadata = new("medieval_writing_scroll_smoothing_stone", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6935", []),
		Noun = "stone",
		ShortDescription = "a scroll smoothing stone",
		LongDescription = null,
		FullDescription = "This scroll smoothing stone is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "stone",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll Smoothing Stone"
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

	public static CatalogueItemDefinition MedievalWritingScrollEndKnob() => new()
	{
		Metadata = new("medieval_writing_scroll_end_knob", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6962", []),
		Noun = "knob",
		ShortDescription = "a scroll end knob",
		LongDescription = null,
		FullDescription = "This scroll end knob is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll End Knob"
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

	public static CatalogueItemDefinition MedievalWritingScrollTieRibbon() => new()
	{
		Metadata = new("medieval_writing_scroll_tie_ribbon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:6989", []),
		Noun = "ribbon",
		ShortDescription = "a scroll tie ribbon",
		LongDescription = null,
		FullDescription = "This scroll tie ribbon is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8.0,
		Cost = 0.6m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll Tie Ribbon"
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

	public static CatalogueItemDefinition MedievalWritingScrollSealCord() => new()
	{
		Metadata = new("medieval_writing_scroll_seal_cord", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7016", []),
		Noun = "cord",
		ShortDescription = "a scroll seal cord",
		LongDescription = null,
		FullDescription = "This scroll seal cord is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll Seal Cord"
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

	public static CatalogueItemDefinition MedievalWritingScrollLabelTab() => new()
	{
		Metadata = new("medieval_writing_scroll_label_tab", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7043", []),
		Noun = "tab",
		ShortDescription = "a scroll label tab",
		LongDescription = null,
		FullDescription = "This scroll label tab is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.0,
		Cost = 0.8m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll Label Tab"
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

	public static CatalogueItemDefinition MedievalWritingBlockCarvingKnife() => new()
	{
		Metadata = new("medieval_writing_block_carving_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7070", []),
		Noun = "knife",
		ShortDescription = "a block carving knife",
		LongDescription = null,
		FullDescription = "This block carving knife is a small sharp tool for cutting lines into woodblock faces. The working edge is narrow and controlled, made for careful relief carving rather than heavy chopping. It belongs with block preparation, correction, and fine carving work before printing.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Block Carving Knife"
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

	public static CatalogueItemDefinition MedievalWritingBlockClearingChisel() => new()
	{
		Metadata = new("medieval_writing_block_clearing_chisel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7097", []),
		Noun = "chisel",
		ShortDescription = "a block clearing chisel",
		LongDescription = null,
		FullDescription = "This block clearing chisel is a small sharp tool for cutting lines into woodblock faces. The working edge is narrow and controlled, made for careful relief carving rather than heavy chopping. It belongs with block preparation, correction, and fine carving work before printing.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 110.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Block Clearing Chisel"
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

	public static CatalogueItemDefinition MedievalWritingWoodblockInkDauber() => new()
	{
		Metadata = new("medieval_writing_woodblock_ink_dauber", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7124", []),
		Noun = "dauber",
		ShortDescription = "a woodblock ink dauber",
		LongDescription = null,
		FullDescription = "This woodblock ink dauber is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Ink Dauber"
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

	public static CatalogueItemDefinition MedievalWritingPaperDampeningBrush() => new()
	{
		Metadata = new("medieval_writing_paper_dampening_brush", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7151", []),
		Noun = "brush",
		ShortDescription = "a paper dampening brush",
		LongDescription = null,
		FullDescription = "This paper dampening brush is a broad brush for spreading ink across a carved block or copying surface. The bristles are wide and flexible, meant to cover raised areas without gouging the wood. It is a practical printing tool rather than a fine writing brush.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Paper Dampening Brush"
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

	public static CatalogueItemDefinition MedievalWritingPrintingPastePot() => new()
	{
		Metadata = new("medieval_writing_printing_paste_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7178", []),
		Noun = "pot",
		ShortDescription = "a printing paste pot",
		LongDescription = null,
		FullDescription = "This printing paste pot is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Paste Pot"
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

	public static CatalogueItemDefinition MedievalWritingPrintingBaren() => new()
	{
		Metadata = new("medieval_writing_printing_baren", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7205", []),
		Noun = "baren",
		ShortDescription = "a printing baren",
		LongDescription = null,
		FullDescription = "This printing baren is a hand rubbing pad used to press paper against an inked block. Its rounded face spreads pressure across the sheet without needing a heavy press. It belongs to brush-and-block printing, especially for small sheets and handscroll work.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Printing Baren"
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

	public static CatalogueItemDefinition MedievalWritingImpressionSpoon() => new()
	{
		Metadata = new("medieval_writing_impression_spoon", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7232", []),
		Noun = "spoon",
		ShortDescription = "an impression spoon",
		LongDescription = null,
		FullDescription = "This impression spoon is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Impression Spoon"
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

	public static CatalogueItemDefinition MedievalWritingRegistrationPins() => new()
	{
		Metadata = new("medieval_writing_registration_pins", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7259", []),
		Noun = "pins",
		ShortDescription = "a set of registration pins",
		LongDescription = null,
		FullDescription = "This set of registration pins is a simple guide used to align sheets, blocks, or repeated impressions. Its straight edges and marked positions help keep paper from drifting while copies are made. It supports more consistent woodblock printing and repeated document work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Registration Pin"
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

	public static CatalogueItemDefinition MedievalWritingBlankWoodblock() => new()
	{
		Metadata = new("medieval_writing_blank_woodblock", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7286", []),
		Noun = "block",
		ShortDescription = "a blank printing woodblock",
		LongDescription = null,
		FullDescription = "This blank printing woodblock is a smooth piece of wood left ready for carving into a printing surface. The grain is even enough for fine knife work, and the face has been planed flat before cutting begins. It is an unfinished but useful stock piece for block printing.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
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

	public static CatalogueItemDefinition MedievalWritingCarvedTextBlock() => new()
	{
		Metadata = new("medieval_writing_carved_text_block", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7313", []),
		Noun = "block",
		ShortDescription = "a carved text woodblock",
		LongDescription = null,
		FullDescription = "This carved text woodblock is a flat wooden block prepared for relief printing. The face is smooth and even, with the working surface meant to carry carved text, lines, or image areas. It belongs to woodblock copying, where ink is applied to the raised design and transferred to paper.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1600.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
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

	public static CatalogueItemDefinition MedievalWritingSutraPrintingBlock() => new()
	{
		Metadata = new("medieval_writing_sutra_printing_block", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7340", []),
		Noun = "block",
		ShortDescription = "a sutra printing block",
		LongDescription = null,
		FullDescription = "This sutra printing block is a flat wooden block prepared for relief printing. The face is smooth and even, with the working surface meant to carry carved text, lines, or image areas. It belongs to woodblock copying, where ink is applied to the raised design and transferred to paper.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1700.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
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

	public static CatalogueItemDefinition MedievalWritingBambooPrintingBoard() => new()
	{
		Metadata = new("medieval_writing_bamboo_printing_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7367", []),
		Noun = "board",
		ShortDescription = "a bamboo printing board",
		LongDescription = null,
		FullDescription = "This bamboo printing board is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 980.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
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

	public static CatalogueItemDefinition MedievalWritingWoodblockProofSheet() => new()
	{
		Metadata = new("medieval_writing_woodblock_proof_sheet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7394", []),
		Noun = "sheet",
		ShortDescription = "a woodblock proof sheet",
		LongDescription = null,
		FullDescription = "This woodblock proof sheet is a trial sheet or sample used to check the impression from a block or brush copy. The paper shows the kind of blank or test-ready surface expected before a run of finished copies. It is part of the checking and correction process around block printing.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
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

	public static CatalogueItemDefinition MedievalWritingPrintingPaperStack() => new()
	{
		Metadata = new("medieval_writing_printing_paper_stack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7424", []),
		Noun = "stack",
		ShortDescription = "a stack of printing paper",
		LongDescription = null,
		FullDescription = "This stack of printing paper is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingInkedPrintingPad() => new()
	{
		Metadata = new("medieval_writing_inked_printing_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7455", []),
		Noun = "pad",
		ShortDescription = "an inked printing pad",
		LongDescription = null,
		FullDescription = "This inked printing pad is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Tools / Woodblock Printing Tools",
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

	public static CatalogueItemDefinition MedievalWritingBlockRubbingPad() => new()
	{
		Metadata = new("medieval_writing_block_rubbing_pad", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7482", []),
		Noun = "pad",
		ShortDescription = "a block rubbing pad",
		LongDescription = null,
		FullDescription = "This block rubbing pad is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Tools / Woodblock Printing Tools",
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

	public static CatalogueItemDefinition MedievalWritingPrintRegistrationFrame() => new()
	{
		Metadata = new("medieval_writing_print_registration_frame", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7509", []),
		Noun = "frame",
		ShortDescription = "a print registration frame",
		LongDescription = null,
		FullDescription = "This print registration frame is a simple guide used to align sheets, blocks, or repeated impressions. Its straight edges and marked positions help keep paper from drifting while copies are made. It supports more consistent woodblock printing and repeated document work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1600.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
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

	public static CatalogueItemDefinition MedievalWritingPrintDryingLine() => new()
	{
		Metadata = new("medieval_writing_print_drying_line", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7536", []),
		Noun = "line",
		ShortDescription = "a print drying line",
		LongDescription = null,
		FullDescription = "This print drying line is a light rack for holding freshly printed or brushed sheets while they dry. The supports keep paper separated so wet ink does not smear or transfer to the next leaf. It is useful in copying rooms, schools, and woodblock workshops.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 220.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Tools / Woodblock Printing Tools",
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

	public static CatalogueItemDefinition MedievalWritingScribesSlopedBoard() => new()
	{
		Metadata = new("medieval_writing_scribes_sloped_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7563", []),
		Noun = "board",
		ShortDescription = "a scribe's sloped board",
		LongDescription = null,
		FullDescription = "This scribe's sloped board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPortableLapDesk() => new()
	{
		Metadata = new("medieval_writing_portable_lap_desk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7593", []),
		Noun = "desk",
		ShortDescription = "a portable lap desk",
		LongDescription = null,
		FullDescription = "This portable lap desk provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3200.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingDeskDrawers() => new()
	{
		Metadata = new("medieval_writing_desk_drawers", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7623", []),
		Noun = "desk",
		ShortDescription = "a small writing desk",
		LongDescription = null,
		FullDescription = "This small writing desk provides a stable surface for books, tablets, or loose sheets. The oak body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 12000.0,
		Cost = 85.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Drawers"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCopyistsBoard() => new()
	{
		Metadata = new("medieval_writing_copyists_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7653", []),
		Noun = "board",
		ShortDescription = "a copyist's writing board",
		LongDescription = null,
		FullDescription = "This copyist's writing board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingManuscriptRest() => new()
	{
		Metadata = new("medieval_writing_manuscript_rest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7683", []),
		Noun = "rest",
		ShortDescription = "a small manuscript rest",
		LongDescription = null,
		FullDescription = "This small manuscript rest provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1400.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPortableBookStand() => new()
	{
		Metadata = new("medieval_writing_portable_book_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7713", []),
		Noun = "stand",
		ShortDescription = "a portable book stand",
		LongDescription = null,
		FullDescription = "This portable book stand provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2200.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingAccountTableBoard() => new()
	{
		Metadata = new("medieval_writing_account_table_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7743", []),
		Noun = "board",
		ShortDescription = "an account table board",
		LongDescription = null,
		FullDescription = "This account table board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 20.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Medieval_Practice_Board_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSchoolTabletRack() => new()
	{
		Metadata = new("medieval_writing_school_tablet_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7773", []),
		Noun = "rack",
		ShortDescription = "a school tablet rack",
		LongDescription = null,
		FullDescription = "This school tablet rack is a small portable storage support for manuscripts, scrolls, tablets, or document cases. The open structure keeps records upright or separated so they can be found quickly. It is suited to archive rooms, classrooms, scriptoria, or temporary workspaces.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4200.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingNarrowArchiveTray() => new()
	{
		Metadata = new("medieval_writing_narrow_archive_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7803", []),
		Noun = "tray",
		ShortDescription = "a narrow archive tray",
		LongDescription = null,
		FullDescription = "This narrow archive tray is a shallow working tray sized for papers, seals, pens, or small document tools. Its raised edges keep small pieces from sliding away while leaving the contents visible and easy to sort. It is meant for desk work, archive sorting, sealing, or manuscript preparation.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingScribeToolRoll() => new()
	{
		Metadata = new("medieval_writing_scribe_tool_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7833", []),
		Noun = "roll",
		ShortDescription = "a scribe's tool roll",
		LongDescription = null,
		FullDescription = "This scribe's tool roll is a flexible tool roll with spaces for small writing implements. The material folds around pens, knives, brushes, or other narrow tools and can be tied into a compact bundle. It suits a travelling scribe, calligrapher, clerk, or copyist who needs portable equipment.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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

	public static CatalogueItemDefinition MedievalWritingCalligrapherToolRoll() => new()
	{
		Metadata = new("medieval_writing_calligrapher_tool_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7863", []),
		Noun = "roll",
		ShortDescription = "a calligrapher's tool roll",
		LongDescription = null,
		FullDescription = "This calligrapher's tool roll is a flexible tool roll with spaces for small writing implements. The material folds around pens, knives, brushes, or other narrow tools and can be tied into a compact bundle. It suits a travelling scribe, calligrapher, clerk, or copyist who needs portable equipment.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 180.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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

	public static CatalogueItemDefinition MedievalWritingCopyistKitBox() => new()
	{
		Metadata = new("medieval_writing_copyist_kit_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7893", []),
		Noun = "box",
		ShortDescription = "a copyist's kit box",
		LongDescription = null,
		FullDescription = "This copyist's kit box is a compact storage box for writing tools, seals, ink goods, or manuscript supplies. Its interior is arranged for small working objects rather than general household storage, and the exterior is built for repeated handling. It belongs with clerks, copyists, messengers, and travelling scribes.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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

	public static CatalogueItemDefinition MedievalWritingClerkWritingBox() => new()
	{
		Metadata = new("medieval_writing_clerk_writing_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7923", []),
		Noun = "box",
		ShortDescription = "a clerk's writing box",
		LongDescription = null,
		FullDescription = "This clerk's writing box is a compact storage box for writing tools, seals, ink goods, or manuscript supplies. Its interior is arranged for small working objects rather than general household storage, and the exterior is built for repeated handling. It belongs with clerks, copyists, messengers, and travelling scribes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Drawers"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingTravellingScribeBox() => new()
	{
		Metadata = new("medieval_writing_travelling_scribe_box", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7953", []),
		Noun = "box",
		ShortDescription = "a travelling scribe's box",
		LongDescription = null,
		FullDescription = "This travelling scribe's box is a compact storage box for writing tools, seals, ink goods, or manuscript supplies. Its interior is arranged for small working objects rather than general household storage, and the exterior is built for repeated handling. It belongs with clerks, copyists, messengers, and travelling scribes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2900.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Drawers"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingScholarsBookBoard() => new()
	{
		Metadata = new("medieval_writing_scholars_book_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:7983", []),
		Noun = "board",
		ShortDescription = "a scholar's book board",
		LongDescription = null,
		FullDescription = "This scholar's book board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2100.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingStandingCopyBoard() => new()
	{
		Metadata = new("medieval_writing_standing_copy_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:8013", []),
		Noun = "board",
		ShortDescription = "a standing copy board",
		LongDescription = null,
		FullDescription = "This standing copy board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingDocumentSortingTray() => new()
	{
		Metadata = new("medieval_writing_document_sorting_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:8043", []),
		Noun = "tray",
		ShortDescription = "a document sorting tray",
		LongDescription = null,
		FullDescription = "This document sorting tray is a shallow working tray sized for papers, seals, pens, or small document tools. Its raised edges keep small pieces from sliding away while leaving the contents visible and easy to sort. It is meant for desk work, archive sorting, sealing, or manuscript preparation.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 800.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSealImpressionTray() => new()
	{
		Metadata = new("medieval_writing_seal_impression_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:8073", []),
		Noun = "tray",
		ShortDescription = "a seal impression tray",
		LongDescription = null,
		FullDescription = "This seal impression tray is a shallow working tray sized for papers, seals, pens, or small document tools. Its raised edges keep small pieces from sliding away while leaving the contents visible and easy to sort. It is meant for desk work, archive sorting, sealing, or manuscript preparation.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 850.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingInkAndPenTray() => new()
	{
		Metadata = new("medieval_writing_ink_and_pen_tray", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:8103", []),
		Noun = "tray",
		ShortDescription = "an ink and pen tray",
		LongDescription = null,
		FullDescription = "This ink and pen tray is a shallow working tray sized for papers, seals, pens, or small document tools. Its raised edges keep small pieces from sliding away while leaving the contents visible and easy to sort. It is meant for desk work, archive sorting, sealing, or manuscript preparation.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 600.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSmallManuscriptShelf() => new()
	{
		Metadata = new("medieval_writing_small_manuscript_shelf", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:8133", []),
		Noun = "shelf",
		ShortDescription = "a small manuscript shelf",
		LongDescription = null,
		FullDescription = "This small manuscript shelf is a small portable storage support for manuscripts, scrolls, tablets, or document cases. The open structure keeps records upright or separated so they can be found quickly. It is suited to archive rooms, classrooms, scriptoria, or temporary workspaces.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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

	public static CatalogueItemDefinition MedievalWritingPortableScrollRack() => new()
	{
		Metadata = new("medieval_writing_portable_scroll_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:8163", []),
		Noun = "rack",
		ShortDescription = "a portable scroll rack",
		LongDescription = null,
		FullDescription = "This portable scroll rack is a flexible tool roll with spaces for small writing implements. The material folds around pens, knives, brushes, or other narrow tools and can be tied into a compact bundle. It suits a travelling scribe, calligrapher, clerk, or copyist who needs portable equipment.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4800.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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

	public static CatalogueItemDefinition MedievalWritingWaxTabletStand() => new()
	{
		Metadata = new("medieval_writing_wax_tablet_stand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:8193", []),
		Noun = "stand",
		ShortDescription = "a wax-tablet stand",
		LongDescription = null,
		FullDescription = "This wax-tablet stand provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
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
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLecternBoard() => new()
	{
		Metadata = new("medieval_writing_lectern_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:8223", []),
		Noun = "board",
		ShortDescription = "a portable lectern board",
		LongDescription = null,
		FullDescription = "This portable lectern board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 40.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryRing01() => new()
	{
		Metadata = new("preindustrial_jewellery_ring_01", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:15", []),
		Noun = "ring",
		ShortDescription = "a shell carnelian ring",
		LongDescription = null,
		FullDescription = "The carnelian ring is worked into a shell pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carnelian",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Rings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Ring", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewellerySignetRing02() => new()
	{
		Metadata = new("preindustrial_jewellery_signet_ring_02", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:38", []),
		Noun = "ring",
		ShortDescription = "a reed lapis lazuli ring",
		LongDescription = null,
		FullDescription = "The lapis lazuli ring is worked into a reed pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 27.0,
		Cost = 17.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lapis lazuli",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Rings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Ring", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryPendant03() => new()
	{
		Metadata = new("preindustrial_jewellery_pendant_03", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:61", []),
		Noun = "pendant",
		ShortDescription = "a rosette turquoise pendant",
		LongDescription = null,
		FullDescription = "The turquoise pendant is worked into a rosette pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 34.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "turquoise",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Pendants", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Necklace", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBeadNecklace04() => new()
	{
		Metadata = new("preindustrial_jewellery_bead_necklace_04", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:84", []),
		Noun = "necklace",
		ShortDescription = "a checker jade necklace",
		LongDescription = null,
		FullDescription = "The jade necklace is worked into a checker pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 41.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Bead Strings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Necklace", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryChainNecklace05() => new()
	{
		Metadata = new("preindustrial_jewellery_chain_necklace_05", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:107", []),
		Noun = "necklace",
		ShortDescription = "a braided quartz necklace",
		LongDescription = null,
		FullDescription = "The quartz necklace is worked into a braided pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 48.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "quartz",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Necklaces", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Necklace", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBracelet06() => new()
	{
		Metadata = new("preindustrial_jewellery_bracelet_06", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:130", []),
		Noun = "bracelet",
		ShortDescription = "a radiating faience bracelet",
		LongDescription = null,
		FullDescription = "The faience bracelet is worked into a radiating pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 55.0,
		Cost = 53.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "faience",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Bracelets", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Bracelet", "Destroyable_Glassware"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryArmlet07() => new()
	{
		Metadata = new("preindustrial_jewellery_armlet_07", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:153", []),
		Noun = "armlet",
		ShortDescription = "a leaf-scroll pewter armlet",
		LongDescription = null,
		FullDescription = "The pewter armlet is worked into a leaf-scroll pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 62.0,
		Cost = 62.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Armlets", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Armlet", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryAnklet08() => new()
	{
		Metadata = new("preindustrial_jewellery_anklet_08", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:176", []),
		Noun = "anklet",
		ShortDescription = "a sunburst copper anklet",
		LongDescription = null,
		FullDescription = "The copper anklet is worked into a sunburst pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Poor,
		WeightInGrams = 69.0,
		Cost = 71.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Anklets", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Anklet", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryEarrings09() => new()
	{
		Metadata = new("preindustrial_jewellery_earrings_09", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:199", []),
		Noun = "earrings",
		ShortDescription = "a water-wave bronze earrings",
		LongDescription = null,
		FullDescription = "The bronze earrings is worked into a water-wave pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 76.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Earrings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Earrings", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBrooch10() => new()
	{
		Metadata = new("preindustrial_jewellery_brooch_10", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:222", []),
		Noun = "brooch",
		ShortDescription = "a knotted silver brooch",
		LongDescription = null,
		FullDescription = "The silver brooch is worked into a knotted pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 83.0,
		Cost = 89.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Brooches", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Brooch", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryPin11() => new()
	{
		Metadata = new("preindustrial_jewellery_pin_11", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:245", []),
		Noun = "pin",
		ShortDescription = "a flower-head gold pin",
		LongDescription = null,
		FullDescription = "The gold pin is worked into a flower-head pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 98.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Pins", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Pin", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryHairpin12() => new()
	{
		Metadata = new("preindustrial_jewellery_hairpin_12", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:268", []),
		Noun = "hairpin",
		ShortDescription = "a star brass hairpin",
		LongDescription = null,
		FullDescription = "The brass hairpin is worked into a star pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20.0,
		Cost = 107.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Hair Ornaments", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Hairpin", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryCirclet13() => new()
	{
		Metadata = new("preindustrial_jewellery_circlet_13", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:291", []),
		Noun = "circlet",
		ShortDescription = "a bird glass circlet",
		LongDescription = null,
		FullDescription = "The glass circlet is worked into a bird pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 27.0,
		Cost = 116.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Circlets", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Circlet", "Destroyable_Glassware"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryDiadem14() => new()
	{
		Metadata = new("preindustrial_jewellery_diadem_14", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:314", []),
		Noun = "diadem",
		ShortDescription = "a vine bone diadem",
		LongDescription = null,
		FullDescription = "The bone diadem is worked into a vine pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 34.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Diadems", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Diadem", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryWaistChain15() => new()
	{
		Metadata = new("preindustrial_jewellery_waist_chain_15", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:337", []),
		Noun = "waist chain",
		ShortDescription = "a geometric shell waist chain",
		LongDescription = null,
		FullDescription = "The shell waist chain is worked into a geometric pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 41.0,
		Cost = 134.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "shell",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Waist Chains", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Waist_Chain", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBeltOrnament16() => new()
	{
		Metadata = new("preindustrial_jewellery_belt_ornament_16", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:360", []),
		Noun = "belt ornament",
		ShortDescription = "a cloud ivory belt ornament",
		LongDescription = null,
		FullDescription = "The ivory belt ornament is worked into a cloud pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Poor,
		WeightInGrams = 48.0,
		Cost = 143.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivory",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Belt Ornaments", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Belt_Ornament", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBadge17() => new()
	{
		Metadata = new("preindustrial_jewellery_badge_17", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:383", []),
		Noun = "badge",
		ShortDescription = "a shell pearl badge",
		LongDescription = null,
		FullDescription = "The pearl badge is worked into a shell pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 152.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Badges", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Badge", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryTempleRing18() => new()
	{
		Metadata = new("preindustrial_jewellery_temple_ring_18", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:406", []),
		Noun = "temple ring",
		ShortDescription = "a reed amber temple ring",
		LongDescription = null,
		FullDescription = "The amber temple ring is worked into a reed pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 62.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "amber",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Temple Rings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Temple_Rings", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryTorc19() => new()
	{
		Metadata = new("preindustrial_jewellery_torc_19", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:429", []),
		Noun = "torc",
		ShortDescription = "a rosette coral torc",
		LongDescription = null,
		FullDescription = "The coral torc is worked into a rosette pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 69.0,
		Cost = 17.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "coral",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Torcs", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Torc", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryWreath20() => new()
	{
		Metadata = new("preindustrial_jewellery_wreath_20", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:452", []),
		Noun = "wreath",
		ShortDescription = "a checker garnet wreath",
		LongDescription = null,
		FullDescription = "The garnet wreath is worked into a checker pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 76.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "garnet",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Wreaths", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Wreath", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryRing21() => new()
	{
		Metadata = new("preindustrial_jewellery_ring_21", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:475", []),
		Noun = "ring",
		ShortDescription = "a braided agate ring",
		LongDescription = null,
		FullDescription = "The agate ring is worked into a braided pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 83.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "agate",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Rings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Ring", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewellerySignetRing22() => new()
	{
		Metadata = new("preindustrial_jewellery_signet_ring_22", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:498", []),
		Noun = "ring",
		ShortDescription = "a radiating carnelian ring",
		LongDescription = null,
		FullDescription = "The carnelian ring is worked into a radiating pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 90.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carnelian",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Rings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Ring", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryPendant23() => new()
	{
		Metadata = new("preindustrial_jewellery_pendant_23", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:521", []),
		Noun = "pendant",
		ShortDescription = "a leaf-scroll lapis lazuli pendant",
		LongDescription = null,
		FullDescription = "The lapis lazuli pendant is worked into a leaf-scroll pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20.0,
		Cost = 53.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lapis lazuli",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Pendants", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Necklace", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBeadNecklace24() => new()
	{
		Metadata = new("preindustrial_jewellery_bead_necklace_24", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:544", []),
		Noun = "necklace",
		ShortDescription = "a sunburst turquoise necklace",
		LongDescription = null,
		FullDescription = "The turquoise necklace is worked into a sunburst pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Poor,
		WeightInGrams = 27.0,
		Cost = 62.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "turquoise",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Bead Strings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Necklace", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryChainNecklace25() => new()
	{
		Metadata = new("preindustrial_jewellery_chain_necklace_25", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:567", []),
		Noun = "necklace",
		ShortDescription = "a water-wave jade necklace",
		LongDescription = null,
		FullDescription = "The jade necklace is worked into a water-wave pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34.0,
		Cost = 71.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Necklaces", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Necklace", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBracelet26() => new()
	{
		Metadata = new("preindustrial_jewellery_bracelet_26", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:590", []),
		Noun = "bracelet",
		ShortDescription = "a knotted quartz bracelet",
		LongDescription = null,
		FullDescription = "The quartz bracelet is worked into a knotted pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 41.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "quartz",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Bracelets", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Bracelet", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryArmlet27() => new()
	{
		Metadata = new("preindustrial_jewellery_armlet_27", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:613", []),
		Noun = "armlet",
		ShortDescription = "a flower-head faience armlet",
		LongDescription = null,
		FullDescription = "The faience armlet is worked into a flower-head pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 48.0,
		Cost = 89.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "faience",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Armlets", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Armlet", "Destroyable_Glassware"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryAnklet28() => new()
	{
		Metadata = new("preindustrial_jewellery_anklet_28", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:636", []),
		Noun = "anklet",
		ShortDescription = "a star pewter anklet",
		LongDescription = null,
		FullDescription = "The pewter anklet is worked into a star pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 98.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Anklets", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Anklet", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryEarrings29() => new()
	{
		Metadata = new("preindustrial_jewellery_earrings_29", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:659", []),
		Noun = "earrings",
		ShortDescription = "a bird copper earrings",
		LongDescription = null,
		FullDescription = "The copper earrings is worked into a bird pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 62.0,
		Cost = 107.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Earrings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Earrings", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBrooch30() => new()
	{
		Metadata = new("preindustrial_jewellery_brooch_30", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:682", []),
		Noun = "brooch",
		ShortDescription = "a vine bronze brooch",
		LongDescription = null,
		FullDescription = "The bronze brooch is worked into a vine pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 69.0,
		Cost = 116.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Brooches", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Brooch", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryPin31() => new()
	{
		Metadata = new("preindustrial_jewellery_pin_31", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:705", []),
		Noun = "pin",
		ShortDescription = "a geometric silver pin",
		LongDescription = null,
		FullDescription = "The silver pin is worked into a geometric pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 76.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Pins", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Pin", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryHairpin32() => new()
	{
		Metadata = new("preindustrial_jewellery_hairpin_32", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:728", []),
		Noun = "hairpin",
		ShortDescription = "a cloud gold hairpin",
		LongDescription = null,
		FullDescription = "The gold hairpin is worked into a cloud pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Poor,
		WeightInGrams = 83.0,
		Cost = 134.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Hair Ornaments", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Hairpin", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryCirclet33() => new()
	{
		Metadata = new("preindustrial_jewellery_circlet_33", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:751", []),
		Noun = "circlet",
		ShortDescription = "a shell brass circlet",
		LongDescription = null,
		FullDescription = "The brass circlet is worked into a shell pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 143.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Circlets", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Circlet", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryDiadem34() => new()
	{
		Metadata = new("preindustrial_jewellery_diadem_34", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:774", []),
		Noun = "diadem",
		ShortDescription = "a reed glass diadem",
		LongDescription = null,
		FullDescription = "The glass diadem is worked into a reed pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 20.0,
		Cost = 152.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Diadems", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Diadem", "Destroyable_Glassware"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryWaistChain35() => new()
	{
		Metadata = new("preindustrial_jewellery_waist_chain_35", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:797", []),
		Noun = "waist chain",
		ShortDescription = "a rosette bone waist chain",
		LongDescription = null,
		FullDescription = "The bone waist chain is worked into a rosette pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 27.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Waist Chains", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Waist_Chain", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBeltOrnament36() => new()
	{
		Metadata = new("preindustrial_jewellery_belt_ornament_36", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:820", []),
		Noun = "belt ornament",
		ShortDescription = "a checker shell belt ornament",
		LongDescription = null,
		FullDescription = "The shell belt ornament is worked into a checker pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34.0,
		Cost = 17.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "shell",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Belt Ornaments", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Belt_Ornament", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBadge37() => new()
	{
		Metadata = new("preindustrial_jewellery_badge_37", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:843", []),
		Noun = "badge",
		ShortDescription = "a braided ivory badge",
		LongDescription = null,
		FullDescription = "The ivory badge is worked into a braided pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 41.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivory",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Badges", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Badge", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryTempleRing38() => new()
	{
		Metadata = new("preindustrial_jewellery_temple_ring_38", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:866", []),
		Noun = "temple ring",
		ShortDescription = "a radiating pearl temple ring",
		LongDescription = null,
		FullDescription = "The pearl temple ring is worked into a radiating pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 48.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pearl",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Temple Rings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Temple_Rings", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryTorc39() => new()
	{
		Metadata = new("preindustrial_jewellery_torc_39", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:889", []),
		Noun = "torc",
		ShortDescription = "a leaf-scroll amber torc",
		LongDescription = null,
		FullDescription = "The amber torc is worked into a leaf-scroll pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "amber",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Torcs", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Torc", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryWreath40() => new()
	{
		Metadata = new("preindustrial_jewellery_wreath_40", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:912", []),
		Noun = "wreath",
		ShortDescription = "a sunburst coral wreath",
		LongDescription = null,
		FullDescription = "The coral wreath is worked into a sunburst pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Poor,
		WeightInGrams = 62.0,
		Cost = 53.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "coral",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Wreaths", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Wreath", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryRing41() => new()
	{
		Metadata = new("preindustrial_jewellery_ring_41", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:935", []),
		Noun = "ring",
		ShortDescription = "a water-wave garnet ring",
		LongDescription = null,
		FullDescription = "The garnet ring is worked into a water-wave pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 69.0,
		Cost = 62.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "garnet",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Rings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Ring", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewellerySignetRing42() => new()
	{
		Metadata = new("preindustrial_jewellery_signet_ring_42", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:958", []),
		Noun = "ring",
		ShortDescription = "a knotted agate ring",
		LongDescription = null,
		FullDescription = "The agate ring is worked into a knotted pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Excellent,
		WeightInGrams = 76.0,
		Cost = 71.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "agate",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Rings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Ring", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryPendant43() => new()
	{
		Metadata = new("preindustrial_jewellery_pendant_43", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:981", []),
		Noun = "pendant",
		ShortDescription = "a flower-head carnelian pendant",
		LongDescription = null,
		FullDescription = "The carnelian pendant is worked into a flower-head pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 83.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "carnelian",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Pendants", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Necklace", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBeadNecklace44() => new()
	{
		Metadata = new("preindustrial_jewellery_bead_necklace_44", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:1004", []),
		Noun = "necklace",
		ShortDescription = "a star lapis lazuli necklace",
		LongDescription = null,
		FullDescription = "The lapis lazuli necklace is worked into a star pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 89.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "lapis lazuli",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Bead Strings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Necklace", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryChainNecklace45() => new()
	{
		Metadata = new("preindustrial_jewellery_chain_necklace_45", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:1027", []),
		Noun = "necklace",
		ShortDescription = "a bird turquoise necklace",
		LongDescription = null,
		FullDescription = "The turquoise necklace is worked into a bird pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 20.0,
		Cost = 98.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "turquoise",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Necklaces", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Necklace", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBracelet46() => new()
	{
		Metadata = new("preindustrial_jewellery_bracelet_46", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:1050", []),
		Noun = "bracelet",
		ShortDescription = "a vine jade bracelet",
		LongDescription = null,
		FullDescription = "The jade bracelet is worked into a vine pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 27.0,
		Cost = 107.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jade",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Bracelets", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Bracelet", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryArmlet47() => new()
	{
		Metadata = new("preindustrial_jewellery_armlet_47", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:1073", []),
		Noun = "armlet",
		ShortDescription = "a geometric quartz armlet",
		LongDescription = null,
		FullDescription = "The quartz armlet is worked into a geometric pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34.0,
		Cost = 116.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "quartz",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Armlets", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Armlet", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryAnklet48() => new()
	{
		Metadata = new("preindustrial_jewellery_anklet_48", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:1096", []),
		Noun = "anklet",
		ShortDescription = "a cloud faience anklet",
		LongDescription = null,
		FullDescription = "The faience anklet is worked into a cloud pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Poor,
		WeightInGrams = 41.0,
		Cost = 125.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "faience",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Anklets", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Anklet", "Destroyable_Glassware"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryEarrings49() => new()
	{
		Metadata = new("preindustrial_jewellery_earrings_49", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:1119", []),
		Noun = "earrings",
		ShortDescription = "a shell pewter earrings",
		LongDescription = null,
		FullDescription = "The pewter earrings is worked into a shell pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 48.0,
		Cost = 134.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pewter",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Earrings", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Earrings", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition PreindustrialJewelleryBrooch50() => new()
	{
		Metadata = new("preindustrial_jewellery_brooch_50", "shared-preindustrial", ["antiquity", "earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialBaseline.JewelleryDoors.Generated.cs:1142", []),
		Noun = "brooch",
		ShortDescription = "a reed copper brooch",
		LongDescription = null,
		FullDescription = "The copper brooch is worked into a reed pattern with smooth inner surfaces and visible tool marks at the joins. Its fastening is plainly made for ordinary wear and does not conceal a mechanism.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 55.0,
		Cost = 143.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = ["Era / Pre-Industrial Era", "Functions / Worn Items / Jewellery", "Functions / Worn Items / Jewellery / Brooches", "Market / Jewellery / Standard Jewellery"],
		Components = ["Holdable", "Wear_Brooch", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Pre-industrial personal adornment catalogue; culture admission cross-cultural; availability Antiquity / Medieval / Renaissance / Early Modern. Description is form-led and makes no unsupported devotional or mechanical claim.",
		AllowLegacyShortDescriptionMatch = false,
	};
}
